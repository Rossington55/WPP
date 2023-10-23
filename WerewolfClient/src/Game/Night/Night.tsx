/*
    PROPS
    --------------
      
 */

import React, { useContext, useState, useEffect } from 'react';
import { Role, RoleContext, Team } from '../Game';
import { Button, Chip, List, ListItem, ListItemSuffix } from '@material-tailwind/react';
import { SocketContext } from '../../App';

interface Props {
    players: Array<string>,
    done: boolean
}

interface Player {
    name: string,
    selectedCount: number,
    selectedByMe: boolean
}

export default function Night(props: Props) {
    const [players, setPlayers] = useState<Array<Player>>([])
    const myName = sessionStorage.getItem("name")
    const socket = useContext(SocketContext)
    const role = useContext(RoleContext)

    useEffect(() => {
        switch (socket.recieved.action) {
            case "WerewolfClick":
                handleWerewolfSelected()
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
        if (!role?.canMultiClick) {
            //Only select one at a time
            for (let player of newPlayers) {
                if (player.selectedByMe) {
                    socket.send(`WerewolfClick;${myName};${player.name};false`)
                }
                player.selectedByMe = false
            }
        }

        const player = newPlayers[i]

        newPlayers[i].selectedByMe = !player.selectedByMe
        //WWClick;MyName;SelectedName;Did Select
        socket.send(`WerewolfClick;${myName};${player.name};${player.selectedByMe}`)

        setPlayers(newPlayers)
    }

    function handleWerewolfSelected() {
        let playersCopy = [...players]
        const selected = !socket.recieved.value.includes("!")
        let i = players.findIndex(player =>
            player.name === socket.recieved.value.replace("!", "")
        )

        //Didnt find any players
        if (i == -1) { return }

        if (selected) {
            playersCopy[i].selectedCount++
        } else {//Deselected
            playersCopy[i].selectedCount--
        }

        setPlayers(playersCopy)
    }

    function handleReady() {
        let output = `NightSubmit;${myName};`
        const selectedPlayers = players.filter(player => player.selectedByMe)
        selectedPlayers.map(player => {
            output += player.name
            output += ","
        })

        output = output.substring(0, output.length - 1)
        socket.send(output)
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


    return (
        <article>
            <h1>NIGHT TIME</h1>

            {!props.done ?
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
                            onClick={handleReady}
                        >
                            Ready
                        </Button>
                    }
                </article>
                :
                <article>
                    Done for the night
                </article>
            }
        </article>
    );
}
