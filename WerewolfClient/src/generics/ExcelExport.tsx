/*
    PROPS
    --------------
      
 */

import React from 'react';
import { useExcelDownloder } from 'react-xls';
import { useContext, useEffect, useState } from "react"

export interface ExcelData {
    //Workbook
    [key: string]: Array<{
        //Array of data
        [key: string]: string | number
    }>
}

interface Props {
    filename: string,
    data: any,
    buttonLabel?: string,
}

export default function ExcelExport(props: Props) {
    const { ExcelDownloder, Type, data } = useExcelDownloder();
    const [stateData, setStateData] = useState<any>()

    useEffect(() => {
        setStateData(props.data)
    }, [props.data])

    console.log(data)
    if (stateData === null) {
        return <div />
    }


    return (
        <ExcelDownloder
            data={stateData}
            filename={props.filename}
            type={Type.Button} // or type={'button'}
        >
            {props.buttonLabel ?? "Download .XLSX"}
        </ExcelDownloder>
    );
}
