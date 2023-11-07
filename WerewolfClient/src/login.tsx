import { useState, useContext, useEffect } from 'react'
import BasicField from './generics/fields/BasicField'
import { Button } from '@material-tailwind/react'
import { CommandClient, CommandServer, SocketContext } from './App'
import background from './assets/images/loginBackground.jpg'

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
        <article className='max-h-[100vh] overflow-hidden' >
            <img
                src={background}
                className='opacity-60 max-w-none self-center'
                width={1018}
            />
            <article className='z-1 absolute justify-center h-full w-full p-5'>
                <article className='translate-y-[-25%] gap-8'>
                    <article>
                        <h2 className='self-center text-red-600 text-4xl font-custom1'>Feldman Homebrew</h2>
                        <h1 className='self-center text-red text-8xl font-custom1'>WEREWOLF</h1>
                    </article>
                    <article className='gap-5 mt-28 p-5'>
                        <BasicField
                            object={name}
                            setStateFunc={setName}
                            errMsg={errMsg}
                            label="Your Name"
                        />

                        <Button
                            color="red"
                            className='font-custom2 text-xl'
                            onClick={handleLogin}
                            disabled={loading}
                        >
                            Enter
                        </Button>
                    </article>
                </article>
            </article>
        </article >
    )
}