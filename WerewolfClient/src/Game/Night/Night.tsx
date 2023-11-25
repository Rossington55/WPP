/*
    PROPS
    --------------
      
 */

import { useContext, useState, useEffect } from 'react';
import { RoleContext, Team } from '../Game';
import { Button, Chip, List, ListItem, ListItemSuffix } from '@material-tailwind/react';
import { CommandClient, CommandServer, SocketContext } from '../../App';
import { ALL_ROLE_MEDIA, RoleButton } from '../../generics/Config';
import BGcottage from '../../assets/images/VillagerNight.png'
import BGdoorstep from '../../assets/images/Doorstep.jpg'
import JigglyText from '../../generics/JigglyText';
import { color } from '@material-tailwind/react/types/components/alert';


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
    const [background ,setBackground] = useState<string>("")

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
        const newPlayers: Array<Player> = []
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
        const newPlayers = [...players]


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
        const newPlayers = [...players]
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
        const selectedPlayerNames: Array<string> = []
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

        for (let roleMedia of ALL_ROLE_MEDIA) {
            if (!roleMedia.name.includes(role.name)) {
                continue
            }

            setMyRoleButtons(roleMedia.buttons)
            switch(roleMedia.nightImage){
                case "Cottage":
                    setBackground(BGcottage)
                    break
                case "Doorstep":
                    setBackground(BGdoorstep)
                    break
            }

            break
        }
    }

    function checkReadyDisabled(button: RoleButton): boolean {

        //Werewolf validation
        if (role?.team == Team.Werewolf) {
            const selectedPlayers = players.filter(player => player.selectedCount > 0)

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
            if (button.returnString === "Health") { return sessionStorage.getItem("Used Health") !== "" }
            if (button.returnString === "Poison") { return sessionStorage.getItem("Used Poison") !== "" }
        }

        return players.findIndex(player => player.selectedByMe) == -1
    }

    function getButtonColor(): color {
        switch(role?.team){
            case Team.Villager:
                return "blue"
            case Team.Werewolf:
                return "red"
            case Team.Cult:
                return "green"
            case Team.Tanner:
                return "brown"
            default:
                return 'gray'
        }
    }

    return (
        <article className='overflow-hidden'>
            <img
                src={background}
                className='max-w-none self-center opacity-50'
                width={1000}
            />

            <article className='absolute items-center w-full h-full justify-center'>



            {!submitted ?
                <article className='gap-5'>

                <h1 className='text-center font-custom1 text-5xl'>
<JigglyText text={role?.nightDescription ?? ""}/>
                </h1>

                    {role?.hasNightTask && !role.noNightSelection &&
                        <List>
                            {players.map((player, i) => (
                                <ListItem
                                key={i}
                                selected={player.selectedByMe}
                                onClick={() => handleSelect(i)}
                                disabled={!role.canSelectLast && lastSelectedPlayers.includes(player.name)}
                                className='font-custom2'
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
                                color={getButtonColor()}
                                className={button.className + " font-custom2" }
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
                    <h2>
                    <JigglyText text='Done for the night'/>
                    </h2>

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
            </article>
    );
}

