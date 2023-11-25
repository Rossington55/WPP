import { useEffect, useState } from "react";

/*
    PROPS
    --------------
      
 */
interface Props {
    text: string
}

interface Letter {
    letter: string,
    className: string
}
export default function JigglyText(props: Props) {
    const [text, setText] = useState<Array<Letter>>([])

    useEffect(() => {
        const textArr = props.text.split("")
        const newText = []

        for (let letter of textArr) {
            //Generate random float value
            const rand: number = Math.floor(Math.random() * 2)
            let className: string = "floatText1"
            if (rand == 0) {
                className = "floatText2"
            }
            if (letter === " ") {
                className += " w-4"
            }

            newText.push({
                letter: letter,
                className: className
            })
        }
        setText(newText)
    }, [])
    return (
        <section className="font-custom1">
            {text.map((letter, i) => (
                <div
                    key={i}
                    className={letter.className}
                >
                    {letter.letter}
                </div>
            ))}
        </section>
    );
}
