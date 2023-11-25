import Login from './login'
import Game from './Game/Game'
import { createContext, useState } from 'react'

export enum CommandServer { None, Join, Leave, GetPlayers, Host, Start, RemindState, StartNight, WerewolfSelectPlayer, NightSubmit, SelectVote, SubmitVote, StartDay, Close }
export enum CommandClient { None, Connected, Joined, Left, HostFound, PlayerList, Role, SelectedPlayerList, Submitted, Murdered, State, EndGame, Alert, StartingGame }

interface Socket {
  recieved: SocketMessage,
  send: (msg: SocketMessage) => void
  rejoin: () => void
}

export interface SocketMessage {
  commandServer?: CommandServer,
  commandClient?: CommandClient,
  subCommand?: string,
  player?: string
  data?: Array<string>
}

const emptyMessage: SocketMessage = {
  commandServer: CommandServer.RemindState,
  commandClient: CommandClient.Connected,
  subCommand: "",
  player: "",
  data: []
}

export const SocketContext = createContext<Socket>({ recieved: emptyMessage, send: () => { }, rejoin: () => { } })
let socketObj: WebSocket

function App() {
  const [socketMsg, setSocketMsg] = useState<SocketMessage>(emptyMessage)
  const [joined, setJoined] = useState<boolean>(false)

  console.log("v2")
  return (
    <SocketContext.Provider value={{
      recieved: socketMsg,
      send: (msg) => {
        if (!socketObj || !socketObj.OPEN) { return }

        console.log(`%cSending ${CommandServer[Number(msg.commandServer)]}`, 'color:#5555ff', msg)
        socketObj.send(JSON.stringify(msg))
      },
      rejoin: async () => {
        //Rejoin and redefine socket
        console.log("rejoining")


        // socketObj = await new WebSocket(`wss://werewolf-server.azurewebsites.net/ws`)
        // socketObj = await new WebSocket(`ws://192.168.0.${serverIP}:443/ws`)
        // socketObj = await new WebSocket(`ws://192.168.0.${serverIP}:80/ws`)

        socketObj = await new WebSocket(`ws://localhost:80/ws`)


        socketObj.onopen = () => {
          console.info("Socket open")
        }

        socketObj.onmessage = (data: any) => {
          let msg: SocketMessage = JSON.parse(data.data)
          console.log(`%cRecieved ${CommandClient[Number(msg.commandClient)]}`, 'color:#55ff55', msg)
          setSocketMsg(msg)
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
