/*
    PROPS
    --------------
      
 */

import EntranceBackground from './EntranceBackground';
import JigglyText from '../generics/JigglyText';

interface Props {
    players: Array<string>
}

export default function Lobby(props: Props) {

    return (
        <EntranceBackground fadeIn>
            <article className='absolute h-full w-full'>
                <h1 className='ghostFadeIn self-center mt-5 text-red'>
                    <JigglyText text="LOBBY" />
                </h1>
                <article className='ghostFadeIn max-h-[90%] overflow-scroll backdrop-blur-sm grid gap-5 p-5 grid-cols-2 feather'>
                    {props.players.map((player, i) => (
                        <article
                            key={i}
                            className='ghostFadeIn bg-red-700 rounded-md h-[50px] items-center justify-center'
                        >
                            <h3 className='text-red-200 font-bold font-custom2'>
                                {player}
                            </h3>
                        </article>
                    ))}
                </article>
            </article>
        </EntranceBackground>
    )

}
