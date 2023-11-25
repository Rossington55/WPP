/*
    PROPS
    --------------
      
 */
import nightDeath from "../assets/images/nightDeath.jpg"
import dayDeath from "../assets/images/dayDeath.jpg"
import Fog from "../generics/Fog"

interface Props {
    deathMessage: string
}

export default function DeathScreen(props: Props) {

    return (
        <article className="quickSlideIn h-full w-full bg-black absolute overflow-hidden">
            <article className="fadeIn">

                <img
                    src={props.deathMessage.includes("hung") ? dayDeath : nightDeath}
                    className='max-w-none self-center opacity-50'
                    width={1000}
                />

                <div className="absolute">
                    <Fog />
                </div>
                <article className="absolute w-full h-full p-5 pt-44">
                    <h1
                        className="font-custom1 z-10 text-center text-red"
                        style={{
                            textShadow: "1px 1px 2px white"
                        }}
                    >
                        {props.deathMessage.toUpperCase()}
                    </h1>
                </article>
            </article>
        </article>
    );
}
