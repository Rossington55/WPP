import { useState, useContext, useEffect } from 'react'
import BasicField from './generics/fields/BasicField'
import { Button } from '@material-tailwind/react'
import { SocketContext } from './App'

interface Props {
    onJoin: () => void
}

export default function Login(props: Props) {
    const [name, setName] = useState<string>("")
    const [errMsg, setErrMsg] = useState<string>("")
    const socket = useContext(SocketContext)

    useEffect(() => {
        switch (socket.recieved.action) {
            //If just connected, attempt joining
            case "Connected":
                setTimeout(() => {
                    socket.send(`Join;${name}`)

                }, 1000);
                return
            case "Players":
            case "Joined":
                //Joining successful, go to lobby
                sessionStorage.setItem("name", name)
                props.onJoin()
                return
        }

        const oldName = sessionStorage.getItem("name")
        if (oldName) {
            setName(oldName)
            socket.rejoin()
        }
    },
        [
            socket.recieved,
        ])

    function handleLogin() {
        if (!name) {
            setErrMsg("Name")
            return
        }

        socket.rejoin()
    }

    return (
        <article className='p-5 justify-center h-full'>
            <article className='gap-5'>
                <BasicField
                    object={name}
                    setStateFunc={setName}
                    errMsg={errMsg}
                    label="Name"
                />

                <Button color="blue" onClick={handleLogin}>
                    Join
                </Button>

            </article>
        </article>
    )
}