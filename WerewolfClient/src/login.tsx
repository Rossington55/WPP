import { useState, useContext, useEffect } from 'react'
import BasicField from './generics/fields/BasicField'
import { Button } from '@material-tailwind/react'
import { CommandClient, CommandServer, SocketContext } from './App'

interface Props {
    onJoin: () => void
}

export default function Login(props: Props) {
    const [loading, setLoading] = useState<boolean>(false)
    const [name, setName] = useState<string>("")
    const [errMsg, setErrMsg] = useState<string>("")
    const socket = useContext(SocketContext)

    useEffect(() => {
        switch (socket.recieved.commandClient) {
            //If just connected, attempt joining
            case CommandClient.Connected:
                setTimeout(() => {
                    socket.send({
                        commandServer: CommandServer.Join,
                        player: name
                    })
                }, 1000);
                return
            case CommandClient.Joined:
                //Joining successful, go to lobby
                setLoading(false)
                sessionStorage.setItem("name", name)
                props.onJoin()
                return
        }

    },
        [
            socket.recieved,
        ])

    useEffect(() => {
        const oldName = sessionStorage.getItem("name")
        if (oldName) {
            setLoading(true)
            setName(oldName)

            setTimeout(() => {
                socket.rejoin()
            }, 1000);
        }
    }, [])

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

                <Button
                    color="blue"
                    onClick={handleLogin}
                    disabled={loading}
                >
                    Join
                </Button>

            </article>
        </article>
    )
}