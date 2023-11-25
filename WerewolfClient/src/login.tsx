import { useState, useContext, useEffect } from 'react'
import BasicField from './generics/fields/BasicField'
import { Button } from '@material-tailwind/react'
import { CommandClient, CommandServer, SocketContext } from './App'
import EntranceBackground from './Game/EntranceBackground'
import JigglyText from './generics/JigglyText'

interface Props {
    onJoin: () => void
}

export default function Login(props: Props) {
    const [loading, setLoading] = useState<boolean>(false)
    const [joined, setJoined] = useState<boolean>(false)
    const [name, setName] = useState<string>("")
    const [errMsg, setErrMsg] = useState<string>("")
    const socket = useContext(SocketContext)

    useEffect(() => {
        switch (socket.recieved.commandClient) {
            //If just connected, attempt joining
            case CommandClient.Connected:
                if (name == "") { return }
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
                setJoined(true)
                sessionStorage.setItem("name", name)

                setTimeout(() => {
                    props.onJoin()
                }, 2000)
                return
            case CommandClient.None:
                setLoading(false)
                setJoined(false)
                break
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
            }, 100);
            //Timeout after 3 seconds
            setTimeout(() => {
                if (!joined) {
                    setLoading(false)
                }
            }, 3000)
        }
    }, [])

    function handleLogin() {
        if (!name) {
            setErrMsg("Name")
            return
        }

        setLoading(true)
        socket.rejoin()

        //Timeout after 3 seconds
        setTimeout(() => {
            if (!joined) {
                setLoading(false)
            }
        }, 3000)
    }

    return (
        <EntranceBackground fadeOut={joined}>


            <article className='z-2 absolute justify-around h-full w-full p-5'>
                <article className={joined ? "ghostFadeOut" : ""}>
                    <h2 className='self-center text-red-600 text-4xl font-custom1 floatText2'>Feldman Homebrew</h2>
                    <h1 className='self-center text-red text-8xl'>
                        <JigglyText text="WEREWOLF" />
                    </h1>
                </article>
                <article className={`gap-5 p-5 ${loading || joined ? "ghostFadeOut" : "ghostFadeIn"}`}>
                    <BasicField
                        object={name}
                        setStateFunc={setName}
                        errMsg={errMsg}
                        label="Your Name"
                        maxLength={12}
                    />

                    <Button
                        color="red"
                        className={`font-custom2 text-xl`}
                        onClick={handleLogin}
                        disabled={loading}
                    >
                        Enter
                    </Button>
                </article>
            </article>
        </EntranceBackground>
    )
}