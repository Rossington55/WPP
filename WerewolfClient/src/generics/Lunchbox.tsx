/*
    Set the snack to anything to enable. Will automatically close

    PROPS
    --------------
      snack - {severity, label <optional>}
 */

import React from 'react';
import { Snackbar, Alert, AlertTitle, AlertColor } from '@mui/material';//Inputs
import { useEffect } from 'react';
import { useState } from 'react';

export interface Snack {
    severity: AlertColor | undefined,
    label?: string
}


const colours = {
    "error": "bg-red-300",
    "success": "bg-green-400",
    "info": "bg-green-400",
    "warning": "bg-yellow-400"
}

export default function Lunchbox(props: { snack: Snack | null }) {
    const [snack, setSnack] = useState<Snack | null>(null)

    //Every time the snack object changes
    useEffect(() => {
        //If no snack
        if (!props.snack || props.snack === null) {
            setSnack(null)
            return
        }

        let newSnack: Snack = { ...props.snack }

        //If no label is set, default to the severity as a label
        if (!newSnack.label) {
            newSnack.label = capitalSeverity()
        }

        setSnack(newSnack)
    }, [props.snack])


    function clearSnack() {
        setSnack(null)
    }

    //Replace the first letter with a capital
    function capitalSeverity() {
        const sev = props.snack?.severity ?? ""
        return sev.replace(sev[0], sev[0].toUpperCase())
    }

    if (props.snack === null) {
        return (<div />)
    }

    return (
        <Snackbar
            open={snack !== null}
            onClose={clearSnack}
            anchorOrigin={{ vertical: "top", horizontal: "left" }}
            autoHideDuration={2500}
        >
            <div>
                {snack !== null &&
                    <Alert
                        onClose={clearSnack}
                        severity={snack.severity}
                        className={`items-center flex w-[250px] ${colours[snack.severity ?? "error"]}`}
                    >
                        <AlertTitle className='m-0'>{capitalSeverity()}</AlertTitle>

                        {props.snack.label &&
                            <h3>{snack.label}</h3>
                        }
                    </Alert>
                }
            </div>
        </Snackbar >
    );

}
