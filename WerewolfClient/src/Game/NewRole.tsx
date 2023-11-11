/*
    PROPS
    --------------
      
 */
import { useContext, useState } from 'react'
import { RoleContext, Team } from './Game';
import { Button } from '@material-tailwind/react';
import { color } from '@material-tailwind/react/types/components/alert';

interface Props {
    onClose: () => void
}
export default function NewRole(props: Props) {
    const role = useContext(RoleContext)
    const [closing, setClosing] = useState<boolean>(false)
    const [closed, setClosed] = useState<boolean>(false)

    function handleClick() {
        //Fade out
        setClosing(true)
        setTimeout(() => {
            //Then swipe down
            setClosed(true)
            props.onClose()
        }, 2000)
    }

    function getRoleColor(): { text: string, button: color } {
        switch (role?.team) {
            case Team.Villager:
                return { text: "text-blue", button: "blue" }
            case Team.Werewolf:
                return { text: "text-red", button: "red" }
            case Team.Cult:
                return { text: "text-green", button: "green" }
            case Team.Tanner:
                return { text: "text-brown", button: "brown" }
            default:
                return { text: "text-blue", button: "blue" }
        }
    }

    return (
        <article className={`absolute z-10 bg-black quickSlideIn h-full w-full justify-center p-5 ${closed && "quickSlideOut"}`}>
            <article className={`fadeIn items-center ${getRoleColor().text} ${closing && "fadeOut"}`}>
                <h2 className='font-custom1 text-5xl'>You are the</h2>
                <h1 className='font-custom2 text-5xl'>{role?.name}</h1>

                <h2 className='font-custom1 text-4xl pt-12 text-center'>{role?.description}</h2>

                <Button
                    className='mt-20 w-[300px] p-5'
                    variant='outlined'
                    color={getRoleColor().button}
                    onClick={handleClick}
                >
                    Understood
                </Button>

            </article>
        </article>
    );
}
