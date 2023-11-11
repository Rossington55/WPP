/*
    PROPS
    --------------
      
 */

import { Button } from '@material-tailwind/react';
import { useContext } from 'react';
import { CommandServer, SocketContext } from '../App';
import EntranceBackground from './EntranceBackground';

interface Props {
    players: Array<string>
}

export default function Lobby(props: Props) {
    const socket = useContext(SocketContext)

    return (
        <EntranceBackground fadeIn>
            <article className='absolute h-full w-full'>
                <h1 className='ghostFadeIn font-custom1 self-center mt-5 text-red'>LOBBY</h1>
                <article className='ghostFadeIn max-h-[90%] overflow-scroll backdrop-blur-sm grid gap-5 p-5 grid-cols-2 feather'>
                    {props.players.map((player, i) => (
                        <article
                            key={i}
                            className='ghostFadeIn bg-red-700 rounded-md h-[50px] items-center justify-center'
                        >
                            <h3 className='text-red-200 font-bold font-custom2'>{player}</h3>
                        </article>
                    ))}
                </article>
            </article>
        </EntranceBackground>
    )

    return (
        <article className='gap-2'>
            <h1>Lobby</h1>
            <h2>Players: </h2>
            {props.players.map((player, i) => (
                <p key={i}>{player}</p>
            ))}

            <Button
                color="red"
                onClick={() => {

                    socket.send({
                        commandServer: CommandServer.Leave,
                        player: sessionStorage.getItem("name")?.toString()
                    })
                    sessionStorage.removeItem("name")
                    window.location.reload()
                }}

            >
                Leave
            </Button>
        </article>
    );
}
