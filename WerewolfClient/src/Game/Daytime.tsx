/*
    PROPS
    --------------
      
 */

import { useContext, useEffect, useState } from 'react';
import { CommandClient, CommandServer, SocketContext } from '../App';
import { Button, } from '@material-tailwind/react';
import background from "../assets/images/daytime.png"
import JigglyText from '../generics/JigglyText';
import Fog from '../generics/Fog';

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
    const [submitted, setSubmitted] = useState<boolean>(false)
    const myName = sessionStorage.getItem("name") ?? ""
    const socket = useContext(SocketContext)

    useEffect(() => {
        switch (socket.recieved.commandClient) {
            case CommandClient.SelectedPlayerList:
                populateSelectedPlayers()
                break
            case CommandClient.Submitted:
                setSubmitted(true)
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
    useEffect(() => {
        setSubmitted(false)
    }, [])

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
        //Cant change vote after submitting
        if (submitted) { return }

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
        socket.send({
            commandServer: CommandServer.SelectVote,
            player: myName,
            data: [player.name],
            subCommand: "select"
        })

        setPlayers(newPlayers)
    }

    function submitVote() {
        const selectedPlayer = players.find(player => player.selectedByMe)
        if (!selectedPlayer) { return }

        socket.send({
            commandServer: CommandServer.SubmitVote,
            player: myName,
            data: [selectedPlayer.name]
        })
    }

    return (
        <article className='overflow-hidden'>
            <img
                src={background}
                className='max-w-none self-center opacity-80'
                width={1000}
            />
            <div className={`absolute`}>
                <Fog />
            </div>
            <article className='absolute p-5 justify-around h-full items-center w-full'>
                {/* TITLE */}
                <h1 className='text-center font-custom1 text-5xl'>
                    <JigglyText text="WHO IS A WEREWOLF?" />
                </h1>

                <article className='grid grid-cols-2 gap-5 w-full'>
                    {players.map((player, i) => (

                        <article
                            key={i}
                            className={`ghostFadeIn rounded-md h-[50px] items-center justify-center w-full
                                ${player.selectedByMe ? "bg-red-700" : "bg-white"}
                                `}
                            onClick={() => handleSelect(i)}
                        >
                            <h3 className={` font-bold font-custom2
                            ${player.selectedByMe ? "text-red-400" : "text-black"}
                            `}
                            >
                                {player.name}
                            </h3>
                            {player.selectedCount > 0 &&
                                <article
                                    className='fixed top-0 right-0 translate-x-[50%] translate-y-[-50%] 
                                rounded-[50%] border w-[30px] h-[30px] bg-red 
                                text-center justify-center'
                                >
                                    {player.selectedCount}
                                </article>
                            }
                        </article>
                    ))}
                </article>
                {/* Submit */}
                <Button
                    color="red"
                    className={`font-custom2 text-xl ${submitted && "ghostFadeOut"} w-full`}
                    variant='outlined'
                    disabled={submitted || !players.find(player => player.selectedByMe)}
                    onClick={() => submitVote()}
                >
                    Lock Vote
                </Button>
            </article>
        </article>
    );
}
