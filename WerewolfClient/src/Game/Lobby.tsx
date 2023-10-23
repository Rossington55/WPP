/*
    PROPS
    --------------
      
 */

import { Button } from '@material-tailwind/react';
import React, { useContext } from 'react';
import { SocketContext } from '../App';

interface Props {
    players: Array<string>
}

export default function Lobby(props: Props) {
    const socket = useContext(SocketContext)

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

                    socket.send(`Leave;${sessionStorage.getItem("name")}`)
                    sessionStorage.removeItem("name")
                    window.location.reload()
                }}

            >
                Leave
            </Button>
        </article>
    );
}
