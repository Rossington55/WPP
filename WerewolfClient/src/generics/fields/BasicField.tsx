/*
    Basic Fields
    -------------
    Harrison F    June 2020


    PROPS
    --------------
    name
    label
    type - {select, checkbox, file,buttons, HH:mm, mm:ss}
    value
    className
    errMsg
      
 */

import SelectField from './SelectField';
import { Checkbox, Input, Textarea } from '@material-tailwind/react';
import ButtonField from './ButtonFields';
import moment from 'moment'

export interface Target {
    target: {
        name: string,
        value: string | number | boolean
    }
}


export default function BasicField(props: any) {

    function handleChange(ev: Target) {
        //Handle custom onChange first
        if (props.onChange) {
            props.onChange(ev)
            return
        }

        if (!props.object && props.object !== "") {
            return
        }

        let newObject = { ...props.object }
        if (props.type === "datetime-local") {
            newObject[props.name] = moment(ev.target.value.toLocaleString())
        } else {
            if (props.name) {
                newObject[props.name] = ev.target.value
            } else {
                newObject = ev.target.value
            }

        }
        props.setStateFunc(newObject)
    }

    let inputProps = { ...props }
    if (props.object) {
        if (props.type === "datetime-local") {
            inputProps.value = props.object[props.name].format("yyyy-MM-DDTHH:mm")

        } else {
            if (props.name) {

                inputProps.value = props.object[props.name]
            } else {
                inputProps.value = props.object
            }

        }
    }
    inputProps.onChange = handleChange
    delete inputProps.setStateFunc
    delete inputProps.object

    if (props.type?.includes("hidden")) {
        const requiredMode = props.type.split('&')[1]
        if (props.mode === requiredMode) {
            return <div />
        }
    }

    inputProps.className = inputProps.className + ` ${!props.disabled ? 'peer-placeholder-shown:text-white text-white' : ' peer-placeholder-shown:text-black'}`

    switch (props.type) {
        case "select":
            return <SelectField {...inputProps} />
        case "checkbox":
            return <Checkbox
                {...inputProps}
                onClick={() => handleChange({ target: { name: inputProps.name, value: !inputProps.value } })}
            />

        // <FormControlLabel
        //     sx={{ alignItems: "end" }}
        //     label={<Typography style={{ marginBottom: 10 }}>{props.label}</Typography>}
        //     control={
        //         <Checkbox
        //             checked={inputProps.value === true}
        //             {...inputProps}
        //             color=''
        //             onChange={null}
        //         />
        //     }
        ///>
        case "buttons":
            return <ButtonField {...inputProps} />
        case "textarea":
            return <Textarea
                {...inputProps}
                variant="outlined"
                resize
                color="green"
            />
        default:
            break
    }

    delete inputProps.errMsg

    return (
        <Input
            {...inputProps}
            color='white'
            size='lg'
            error={props.errMsg ? props.errMsg.includes(props.label) : false}
            variant={props.variant ?? "outlined"}
            labelProps={inputProps}
        />
    );
}

