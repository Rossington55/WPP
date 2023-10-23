/*
    PROPS
    --------------
    name
    options - [
        {
            value - value when selected
            label
            colour - {
                main - needed to match material/tailwind
                bg - background
                border
            }
            icon
        }
    ]
 */

import { IconButton } from '@material-tailwind/react';
import { size, color } from '@material-tailwind/react/types/components/button';
import { Tooltip } from '@mui/material';
import React from 'react';
import { Target } from './BasicField';

interface Option {
    value: any,
    label: string,
    icon: any,
    color: {
        main: color,
        bg: string,
        border: string
    }
}

interface Props {
    name: string,
    options: Array<Option>,
    size: size | undefined,
    value: any,
    onChange: (target: Target) => void,
}


export default function ButtonField(props: Props) {

    return (
        <section className='gap-2'>
            {props.options.map((option: Option, i) => (
                <Tooltip
                    key={i}
                    title={option.label}
                >
                    <IconButton
                        size={props.size ?? "lg"}
                        color={option.color.main}

                        //Change style if this is selected
                        className={`${option.value === props.value ? option.color.bg : `bg-transparent border-[1px] ${option.color.border}`}`}

                        /* 
                        return
                        target: {name,value}
                        */
                        onClick={() => props.onChange({ target: { name: props.name, value: option.value } })}
                    >
                        {option.icon}
                    </IconButton>
                </Tooltip>
            ))}
        </section>
    );
}
