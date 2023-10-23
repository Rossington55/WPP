import Login from './login'
import Game from './Game/Game'
import { createContext, useEffect, useState } from 'react'

interface Socket {
  recieved: SocketMessage,
  send: (msg: string) => void
  rejoin: () => void
}

interface SocketMessage {
  action: string,
  value: string,
}

const emptyMessage: SocketMessage = {
  action: "",
  value: "",
}

export const SocketContext = createContext<Socket>({ recieved: emptyMessage, send: () => { }, rejoin: () => { } })
let socketObj: WebSocket

function App() {
  const [socketMsg, setSocketMsg] = useState<SocketMessage>(emptyMessage)
  const [joined, setJoined] = useState<boolean>(false)



  return (
    <SocketContext.Provider value={{
      recieved: socketMsg,
      send: (msg) => {
        console.log(`Sending: ${msg}`)
        socketObj.send(msg)
      },
      rejoin: async () => {
        //Rejoin and redefine socket

        socketObj = await new WebSocket("ws://localhost:8080/ws")

        socketObj.onopen = () => {
          console.info("Socket open")
        }

        socketObj.onmessage = (data: any) => {
          const msg: string = data.data.split(";")
          console.log(`Recieved: ${msg}`)
          setSocketMsg({
            action: msg[0],
            value: msg[1]
          })
        }

        socketObj.onclose = () => {
          console.info("Socket Closing")
        }
      }
    }}>
      {joined ?
        <Game />
        :
        <Login
          onJoin={() => setJoined(true)}
        />
      }
    </SocketContext.Provider>
  )
}

export default App
