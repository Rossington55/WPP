/*
    PROPS
    --------------
      
 */

import React, { useContext, useState, useEffect } from 'react';
import { Role, RoleContext, Team } from '../Game';
import { Button, Chip, List, ListItem, ListItemSuffix } from '@material-tailwind/react';
import { CommandClient, CommandServer, SocketContext } from '../../App';
import { ALL_ROLE_BUTTONS, RoleButton, RoleButtons } from '../../generics/Config';

interface Props {
    players: Array<string>,
}

interface Player {
    name: string,
    selectedCount: number,
    selectedByMe: boolean,
    nextToDeselect: boolean
}


export default function Night(props: Props) {
    const [players, setPlayers] = useState<Array<Player>>([])
    const [submitted, setSubmitted] = useState<boolean>(false)
    const [nightInfo, setNightInfo] = useState<Array<string>>([])
    const [myRoleButtons, setMyRoleButtons] = useState<Array<RoleButton>>([])
    const myName = sessionStorage.getItem("name") ?? ""
    const socket = useContext(SocketContext)
    const role = useContext(RoleContext)

    const lastSelectedPlayers: Array<string> = JSON.parse(sessionStorage.getItem("lastSelectedPlayers") || "[]")

    useEffect(() => {
        switch (socket.recieved.commandClient) {
            case CommandClient.Submitted:
                handleNightInfo()
                break
            case CommandClient.SelectedPlayerList:
                handleOtherWerewolfSelect()
                break
        }
    }, [socket.recieved])

    useEffect(() => {
        let newPlayers: Array<Player> = []
        for (let name of props.players) {
            newPlayers.push({
                name: name,
                selectedByMe: false,
                selectedCount: 0,
                nextToDeselect: false,
            })
        }
        setPlayers(newPlayers)
    }, [props.players])

    useEffect(() => {
        getMyRoleButtons()
    }, [])

    function handleSelect(i: number) {
        let newPlayers = [...players]


        //Multiclick
        if (role?.canMultiClick) {
            for (let player of newPlayers) {
                if (player.nextToDeselect) {
                    player.selectedByMe = false
                    player.nextToDeselect = false
                } else if (player.selectedByMe) {
                    player.nextToDeselect = true
                }
            }

        } else {//Single click
            //First deselect ALL players
            //Only select one at a time
            for (let player of newPlayers) {

                //WEREWOLF CLICK
                if (player.selectedByMe && (role?.team === Team.Werewolf) && (role.name === "Werewolf")) {
                    socket.send({
                        commandServer: CommandServer.WerewolfSelectPlayer,
                        player: myName,
                        data: [player.name],
                        subCommand: "deselect"
                    })
                }
                player.selectedByMe = false
            }

        }

        //Select just this player
        const player = newPlayers[i]

        newPlayers[i].selectedByMe = !player.selectedByMe

        //WEREWOLF CLICK
        if (role?.team == Team.Werewolf && role.name === "Werewolf") {
            socket.send({
                commandServer: CommandServer.WerewolfSelectPlayer,
                player: myName,
                data: [player.name],
                subCommand: "select"
            })
        }

        setPlayers(newPlayers)
    }

    function handleOtherWerewolfSelect() {
        let newPlayers = [...players]
        if (!socket.recieved.data) { return }
        for (let player of socket.recieved.data) {
            const playerData = player.split(';')

            //Find the respective player and update the attack
            const playerId = newPlayers.findIndex(curPlayer => curPlayer.name === playerData[0])
            newPlayers[playerId].selectedCount = Number(playerData[1])
        }

        setPlayers(newPlayers)
    }

    function handleSubmit(buttonClicked: RoleButton) {
        //Create list of selected player names
        let selectedPlayerNames: Array<string> = []
        const selectedPlayers = players.filter(player => player.selectedByMe)
        selectedPlayers.map(player => {
            selectedPlayerNames.push(player.name)
        })

        const subCommand: string = buttonClicked.returnString ?? ""

        socket.send({
            commandServer: CommandServer.NightSubmit,
            subCommand: subCommand,
            player: myName,
            data: selectedPlayerNames
        })

        if (buttonClicked.returnString === "Health") {
            sessionStorage.setItem("Used Health", "true")
        } else if (buttonClicked.returnString === "Poison") {
            sessionStorage.setItem("Used Poison", "true")
        }
    }

    function handleNightInfo() {
        setSubmitted(true)
        const selectedPlayers = players.filter(player => player.selectedByMe).map(player => player.name);
        sessionStorage.setItem("lastSelectedPlayers", JSON.stringify(selectedPlayers));
        if (!socket.recieved.data) { return }
        setNightInfo(socket.recieved.data)
    }

    function getMyRoleButtons() {
        if (!role) { return }

        for (let roleButtons of ALL_ROLE_BUTTONS) {
            if (roleButtons.name.includes(role.name)) {
                setMyRoleButtons(roleButtons.buttons)
            }
        }
    }

    function checkReadyDisabled(button: RoleButton) {

        //Werewolf validation
        if (role?.team == Team.Werewolf) {
            let selectedPlayers = players.filter(player => player.selectedCount > 0)

            //Cant select multiple people
            if (selectedPlayers.length > 1) {
                return true
            }

            //Cant select different people
            if (selectedPlayers.length === 1) {
                return !selectedPlayers[0].selectedByMe
            }
        }

        if (button.requiredSelections || button.requiredSelections === 0) {
            return players.filter(player => player.selectedByMe).length !== button.requiredSelections
        }

        if (role?.name === "Witch") {
            if (button.returnString === "Health") { return sessionStorage.getItem("Used Health") }
            if (button.returnString === "Poison") { return sessionStorage.getItem("Used Poison") }
        }

        return players.findIndex(player => player.selectedByMe) == -1
    }

    return (
        <article>
            <h1>NIGHT TIME</h1>


            {!submitted ?
                <article>

                    <h2>{role?.nightDescription}</h2>

                    {role?.hasNightTask && !role.noNightSelection &&
                        <List>
                            {players.map((player, i) => (
                                <ListItem
                                    key={i}
                                    selected={player.selectedByMe}
                                    onClick={() => handleSelect(i)}
                                    disabled={!role.canSelectLast && lastSelectedPlayers.includes(player.name)}
                                >
                                    {player.name}
                                    <ListItemSuffix>
                                        {player.selectedCount > 0 &&
                                            <Chip
                                                value={player.selectedCount}
                                            />
                                        }
                                    </ListItemSuffix>
                                </ListItem>
                            ))}
                        </List>
                    }

                    {role?.hasNightTask &&
                        <article className='gap-2'>
                            {myRoleButtons.map((button, i) => (
                                <Button
                                    key={i}
                                    disabled={checkReadyDisabled(button)}
                                    color='blue'
                                    className={button.className}
                                    onClick={() => handleSubmit(button)}
                                >
                                    {button.label}
                                </Button>
                            ))}
                        </article>
                    }
                </article>
                :
                <article>
                    Done for the night

                    {nightInfo.length > 0 &&
                        <article className='gap-5'>
                            {/* Result */}
                            {nightInfo.map((info, i) => (
                                <h2 key={i}>{info}</h2>
                            ))}
                        </article>
                    }
                </article>
            }
        </article>
    );
}

