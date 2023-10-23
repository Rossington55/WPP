/*
    Name
    -------------
    Harrison F    June 2020


    

    PROPS
    --------------
    url - data from custom API
    dataKey - name of the object the data is wrapped in
    valueKey - name of the field used for values
    labelKey - name of the field used for labels
    sortKey - if want to custom sort by lookup field, this is the name of the field
    sortNumerical - numerical sort. If not set to true defaults to alphabetical sort

        OR
    tableID - data from lookup table
        OR
    options - Hardcoded options

    

      
 */

import { Option, Select } from '@material-tailwind/react';
import React, { useEffect } from 'react';
import { useState } from 'react';
import { apiGetCall } from '../APIFunctions';
import { capsFirstLetter } from '../GeneralFunctions';

export interface Option {
    value: any,
    label: string,
    id?: string,
}

export default function SelectField(props: any) {
    const [options, setOptions] = useState<Array<Option>>([])

    useEffect(() => {
        if (props.options) {
            //Convert list of strings to object options
            if (typeof (props.options[0] === "string")) {
                let newOptions: Array<Option> = []
                for (let option of props.options) {
                    newOptions.push({
                        label: capsFirstLetter(option),
                        value: option,
                    })
                }
                setOptions(newOptions)
            } else {//Assume already formatted
                setOptions(props.options)
            }
        }

        if (props.tableID) {
            GEToptions()
        } else if (props.url) {
            GETdata()
        }
    }, [props.url, props.options])


    function GEToptions() {
        const url = `Enquiry/lookup/?tableID=${props.tableID}`
        const callback = (data: any) => {
            let newOptions: Array<Option> = []
            data = data.lookupList

            //Get the values and labels of each column
            for (let row of data) {
                newOptions.push({
                    value: row.code,
                    label: row.description,
                })
            }


            setOptions(newOptions)
        }
        const error = (e: string) => {
            console.error(e)
        }
        apiGetCall(url, callback, error)
    }

    function GETdata() {
        const callback = (d: any) => {
            if (props.dataKey) {
                d = d[props.dataKey]
            }



            let newOptions = []

            for (let row of d) {
                newOptions.push({
                    value: row[props.valueKey],
                    label: row[props.labelKey]
                })
            }

            setOptions(newOptions)
        }
        const error = (e: string) => {
            console.error(e)
        }
        apiGetCall(props.url, callback, error)
    }

    const inputProps = { ...props }
    inputProps.className += " text-white"
    delete inputProps.dataKey
    delete inputProps.valueKey
    delete inputProps.labelKey


    return (
        <Select
            {...inputProps}
            onClick={null}
            onChange={null}
            color="blue"
            labelProps={inputProps}
        >
            {options.map((opt, i) => (
                <Option
                    key={i}
                    value={opt.value}
                    onClick={() => props.onChange({ target: { name: props.name, value: opt.value } })}
                >
                    {opt.label}
                </Option>
            ))
            }
        </Select>
    );
}
