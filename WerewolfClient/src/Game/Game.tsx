/*
    PROPS
    --------------
      
 */

import React, { useContext, useEffect, useState, createContext } from 'react';
import { CommandClient, CommandServer, SocketContext } from '../App';
import { Button } from '@material-tailwind/react';
import Lobby from './Lobby';
import Daytime from './Daytime';
import Night from './Night/Night';
import EndGame from './EndGame';
import DeathScreen from './DeathScreen';

enum GameState { Lobby, Daytime, Nighttime, EndGame }
export enum Team { None, Villager, Werewolf, Tanner, Vampire, Cult }

export const RoleContext = createContext<Role | null>(null)

export interface Role {
    name: string,
    description: string,
    nightDescription: string,
    team: Team,
    hasNightTask: boolean,
    canMultiClick: boolean,
    canSelectLast: boolean,
    noNightSelection: boolean,
}

export default function Game() {
    const [done, setDone] = useState<boolean>(false)
    const [players, setPlayers] = useState<Array<string>>([])
    const [gameState, setGameState] = useState<GameState>(GameState.Lobby)
    const [role, setRole] = useState<Role | null>(null)
    const [amHost, setAmHost] = useState<boolean>(false)
    const [winner, setWinner] = useState<string>("")
    const [amDead, setDead] = useState<boolean>(false)
    const socket = useContext(SocketContext)

    useEffect(() => {
        socket.send({
            commandServer: CommandServer.RemindState,
            player: sessionStorage.getItem("name")?.toString()
        })
    }, [])
    useEffect(() => {
        switch (socket.recieved.commandClient) {
            case CommandClient.PlayerList:
                refreshPlayers()
                break
            case CommandClient.Role:
                handleNewRole()
                break
            case CommandClient.State:
                updateState()
                refreshPlayers()
                break
            case CommandClient.HostFound:
                setAmHost(true)
                break
            case CommandClient.EndGame:
                handleEndgame()
                break
            case CommandClient.Murdered:
                setDead(true)
                break
            case CommandClient.Alert:
                handleAlert()
                break
            case CommandClient.Left:
                sessionStorage.removeItem("name")
                window.location.reload()
                break


        }

    }, [socket.recieved])

    function updateState() {
        switch (socket.recieved.subCommand) {
            case "Lobby":
                sessionStorage.removeItem("lastSelectedPlayers")
                setGameState(GameState.Lobby)
                break
            case "Day":
                setDone(false)
                setGameState(GameState.Daytime)
                break
            case "Night":
                setDone(false)
                setGameState(GameState.Nighttime)
                break

        }
    }

    function refreshPlayers() {
        if (socket.recieved.data && socket.recieved.data[0] !== "") {
            setPlayers(socket.recieved.data)
        }
    }

    function handleNewRole() {
        if (!socket.recieved.data) { return }

        setDead(false)
        sessionStorage.removeItem("Used Health")
        sessionStorage.removeItem("Used Poison")
        const roleDetails: Array<string> = socket.recieved.data

        const role: Role = {
            name: roleDetails[0],
            description: roleDetails[1],
            team: Team[roleDetails[2] as keyof typeof Team],
            nightDescription: roleDetails[3],
            hasNightTask: JSON.parse(roleDetails[4].toLowerCase()),
            canMultiClick: JSON.parse(roleDetails[5].toLowerCase()),
            canSelectLast: JSON.parse(roleDetails[6].toLowerCase()),
            noNightSelection: JSON.parse(roleDetails[7].toLowerCase())

        }
        setRole(role)
    }

    function handleEndgame() {
        if (!socket.recieved.data) { return }
        setWinner(socket.recieved.data[0])
        setGameState(GameState.EndGame)
    }

    function handleAlert() {

        if (!socket.recieved.data) { return }
        let output = ""
        for (let item of socket.recieved.data) {
            output += `\n${item}`
        }
        window.alert(output)
    }

    return (
        <article className='p-5 gap-10 h-full w-full justify-between'>
            {amDead ?
                <DeathScreen />
                :
                <RoleContext.Provider value={role}>

                    {gameState === GameState.Lobby &&
                        <Lobby
                            players={players}
                        />
                    }

                    {role &&
                        <>
                            {gameState === GameState.Daytime &&
                                <Daytime
                                    players={players}
                                />
                            }

                            {gameState === GameState.Nighttime &&
                                <Night
                                    players={players}
                                />
                            }
                            {gameState === GameState.EndGame &&
                                <EndGame winner={winner} />
                            }
                        </>
                    }
                </RoleContext.Provider>
            }

            {!amHost ?
                <Button
                    color="blue"
                    onClick={() => socket.send({ commandServer: CommandServer.Host })}
                >
                    Become Host
                </Button>
                :
                <article className='gap-2'>
                    { // gameState === GameState.Lobby &&
                        <Button
                            color="green"
                            onClick={() => socket.send({
                                commandServer: CommandServer.Start,
                                subCommand: "Custom;Werewolf;Mystic Seer"//FOR DEV ONLY
                            })}
                        >
                            Start Game
                        </Button>
                    }
                    {gameState === GameState.Daytime &&
                        <Button
                            color="green"
                            onClick={() => socket.send({ commandServer: CommandServer.StartNight })}
                        >
                            Start Night
                        </Button>
                    }
                    {gameState === GameState.Nighttime &&
                        <Button
                            color="green"
                            onClick={() => socket.send({ commandServer: CommandServer.StartDay })}
                        >
                            Start Day
                        </Button>
                    }
                </article>
            }
        </article>
    );
}


