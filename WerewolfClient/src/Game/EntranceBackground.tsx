/*
    PROPS
    --------------
      
 */

import Fog from '../generics/Fog';
import background from '../assets/images/loginBackground.jpg'
import "../styles/login.css"

interface Props {
    children: React.ReactNode,
    fadeOut?: boolean,
    fadeIn?: boolean
}

export default function EntranceBackground(props: Props) {

    return (
        <article className='max-h-[100vh] overflow-hidden' >
            <img
                id="loginImg"
                src={background}
                className='max-w-none self-center'
                width={1000}
            />
            <div className={`absolute ${props.fadeOut && "fadeOut"} ${props.fadeIn && "fadeIn"}`}>
                <Fog />
            </div>

            {props.children}
        </article >
    );
}
