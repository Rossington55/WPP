/*
    PROPS
    --------------
      
 */

import React, { useContext, useEffect, useState } from 'react';
import MyRole from './MyRole';
import { CommandClient, CommandServer, SocketContext } from '../App';
import { Chip, List, ListItem, ListItemSuffix } from '@material-tailwind/react';

interface Props {
    players: Array<string>,
}

interface Player {
    name: string,
    selectedCount: number,
    selectedByMe: boolean
}

export default function Daytime(props: Props) {
    const [players, setPlayers] = useState<Array<Player>>([])
    const myName = sessionStorage.getItem("name") ?? ""
    const socket = useContext(SocketContext)

    useEffect(() => {
        switch (socket.recieved.commandClient) {
            case CommandClient.SelectedPlayerList:
                populateSelectedPlayers()
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

    function populateSelectedPlayers() {
        let newPlayers = [...players]
        if (!socket.recieved.data) { return }
        for (let player of socket.recieved.data) {
            const playerData = player.split(';')

            //Find the respective player and update the attack
            const playerId = newPlayers.findIndex(curPlayer => curPlayer.name === playerData[0])
            newPlayers[playerId].selectedCount = Number(playerData[1])
            if (socket.recieved.subCommand) {
                newPlayers[playerId].selectedByMe = socket.recieved.subCommand === newPlayers[playerId].name
            }
        }

        setPlayers(newPlayers)
    }
    function handleSelect(i: number) {
        let newPlayers = [...players]

        //Only select one at a time

        //First deselect ALL players
        for (let player of newPlayers) {
            if (player.selectedByMe) {
                socket.send({
                    commandServer: CommandServer.SelectVote,
                    player: myName,
                    data: [player.name],
                    subCommand: "deselect"
                })
                player.selectedByMe = false
            }
        }

        //Select just this player
        const player = newPlayers[i]

        newPlayers[i].selectedByMe = true
        console.log(newPlayers[i])
        socket.send({
            commandServer: CommandServer.SelectVote,
            player: myName,
            data: [player.name],
            subCommand: "select"
        })

        setPlayers(newPlayers)
    }

    return (
        <article>
            It is Daytime
            <MyRole />


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
        </article>
    );
}
