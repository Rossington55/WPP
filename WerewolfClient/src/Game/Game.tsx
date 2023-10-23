/*
    PROPS
    --------------
      
 */

import React, { useContext, useEffect, useState, createContext } from 'react';
import { SocketContext } from '../App';
import { Button } from '@material-tailwind/react';
import Lobby from './Lobby';
import Daytime from './Daytime';
import Night from './Night/Night';

enum GameState { Lobby, Daytime, Nighttime }
export enum Team { Villager, Werewolf, Tanner, Vampire }

export const RoleContext = createContext<Role | null>(null)

export interface Role {
    name: string,
    description: string,
    nightDescription: string,
    team: Team,
    hasNightTask: boolean,
    canMultiClick: boolean,
}

export default function Game() {
    const [done, setDone] = useState<boolean>(false)
    const [players, setPlayers] = useState<Array<string>>([])
    const [gameState, setGameState] = useState<GameState>(GameState.Lobby)
    const [role, setRole] = useState<Role | null>(null)
    const socket = useContext(SocketContext)

    useEffect(() => {
        socket.send(`RemindState;${sessionStorage.getItem("name")}`)
    }, [])

    useEffect(() => {
        switch (socket.recieved.action) {
            case "Players":
                refreshPlayers()
                break
            case "Role":
                handleNewRole()
                setGameState(GameState.Daytime)
                break
            case "State":
                updateState()
                break
            case "NightInfo":
                handleNightInfo()
                break
        }

    }, [socket.recieved])

    function updateState() {
        switch (socket.recieved.value) {
            case "Lobby":
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
        setPlayers(socket.recieved.value.split(","))
    }

    function handleNightInfo() {
        const info = socket.recieved.value
        console.log("Info", info)
        if (info === "Ready") {
            setDone(true)
        }
    }

    function handleNewRole() {
        const roleDetails = socket.recieved.value.split(",")
        const role: Role = {
            name: roleDetails[0],
            description: roleDetails[1],
            team: Team[roleDetails[2] as keyof typeof Team],
            nightDescription: roleDetails[3],
            hasNightTask: JSON.parse(roleDetails[4].toLowerCase()),
            canMultiClick: JSON.parse(roleDetails[5].toLowerCase()),
        }

        setRole(role)
    }

    return (
        <article className='p-5 gap-10 h-full w-full justify-between'>
            <RoleContext.Provider value={role}>
                {gameState === GameState.Lobby &&
                    <Lobby
                        players={players}
                    />
                }


                {gameState === GameState.Daytime &&
                    <Daytime />
                }

                {gameState === GameState.Nighttime &&
                    <Night
                        players={players}
                        done={done}
                    />
                }
            </RoleContext.Provider>
        </article>
    );
}


