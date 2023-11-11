/*
    PROPS
    --------------
      
 */

import { Button, ButtonGroup } from '@material-tailwind/react';

interface Props {
    options: Array<string>,
    selectedOption: string,
    onClick: (selected: string) => void,
}

export default function MultiButton(props: Props) {

    return (
        <ButtonGroup variant='outlined'>
            {props.options.map((option, i) => (
                <Button
                    key={i}
                    className={props.selectedOption === option ? "bg-primary-300 text-white" : "text-primary-50 border-primary-400"}
                    onClick={() => props.onClick(option)}
                >
                    {option}
                </Button>
            ))}
        </ButtonGroup>
    );
}
