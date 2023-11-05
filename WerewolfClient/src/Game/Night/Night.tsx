/*
    PROPS
    --------------
      
 */

import React, { useContext, useState, useEffect } from 'react';
import { Role, RoleContext, Team } from '../Game';
import { Button, Chip, List, ListItem, ListItemSuffix } from '@material-tailwind/react';
import { CommandClient, CommandServer, SocketContext } from '../../App';

interface Props {
    players: Array<string>,
}

interface Player {
    name: string,
    selectedCount: number,
    selectedByMe: boolean
}

export default function Night(props: Props) {
    const [players, setPlayers] = useState<Array<Player>>([])
    const [submitted, setSubmitted] = useState<boolean>(false)
    const [nightInfo, setNightInfo] = useState<string>("")
    const myName = sessionStorage.getItem("name") ?? ""
    const socket = useContext(SocketContext)
    const role = useContext(RoleContext)

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
                selectedCount: 0
            })
        }
        setPlayers(newPlayers)
    }, [props.players])

    function handleSelect(i: number) {
        let newPlayers = [...players]


        //No multiclick
        //First deselect ALL players
        if (!role?.canMultiClick) {
            //Only select one at a time
            for (let player of newPlayers) {

                //WEREWOLF CLICK
                if (player.selectedByMe && role?.team === Team.Werewolf) {
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
        if (role?.team == Team.Werewolf) {
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

    function handleSubmit() {
        //Create list of selected player names
        let selectedPlayerNames: Array<string> = []
        const selectedPlayers = players.filter(player => player.selectedByMe)
        selectedPlayers.map(player => {
            selectedPlayerNames.push(player.name)
        })


        socket.send({
            commandServer: CommandServer.NightSubmit,
            player: myName,
            data: selectedPlayerNames
        })
    }

    function canBeReady() {

        //Werewolf validation
        if (role?.team == Team.Werewolf) {
            let selectedPlayers = players.filter(player => player.selectedCount > 0)

            //Cant select multiple people
            if (selectedPlayers.length > 1) {
                return false
            }

            //Cant select different people
            if (selectedPlayers.length === 1) {
                return selectedPlayers[0].selectedByMe
            }
        }

        return players.findIndex(player => player.selectedByMe) > -1
    }

    function handleNightInfo() {
        setSubmitted(true)
        if (!socket.recieved.data) { return }
        setNightInfo(socket.recieved.data[0])
    }

    return (
        <article>
            <h1>NIGHT TIME</h1>


            {!submitted ?
                <article>

                    <h2>{role?.nightDescription}</h2>

                    {role?.hasNightTask &&
                        <List>
                            {players.map((player, i) => (
                                <ListItem
                                    key={i}
                                    selected={player.selectedByMe}
                                    onClick={() => handleSelect(i)}
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
                        <Button
                            disabled={!canBeReady()}
                            color='blue'
                            onClick={handleSubmit}
                        >
                            Submit
                        </Button>
                    }
                </article>
                :
                <article>
                    Done for the night

                    {nightInfo !== "" &&
                        <article className='gap-5'>

                            {/* Selected Player Names */}
                            <article>
                                <h2>Selected</h2>
                                {players.map((player, i) => (
                                    <>
                                        {player.selectedByMe &&
                                            <h3>{player.name}</h3>
                                        }
                                    </>
                                ))}
                            </article>

                            {/* Result */}
                            <h2>{nightInfo}</h2>
                        </article>
                    }
                </article>
            }
        </article>
    );
}
