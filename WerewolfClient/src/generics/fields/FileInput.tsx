/*

    PROPS
    --------------
 */

import { Close } from '@mui/icons-material';
import { Dialog, IconButton } from '@mui/material';
import React from 'react';
import { useState } from 'react';
import { Target } from './BasicField';

export default function FileInput(props: any) {
    const [file, setFile] = useState<any>(null)
    const [previewOpen, setPreviewOpen] = useState(false)

    function handleCapture(target: any) {
        let file = target.files[0]
        let reader = new FileReader()
        reader.readAsDataURL(file)
        reader.onload = function () {
            file.blob = reader.result
            setFile(file)
            props.onChange({ target: { name: props.name, value: file } })
        }
    }

    let inputProps = { ...props }
    delete inputProps.errMsg

    return (
        <section className='w-full'>
            {/* Input */}
            <input
                className='hidden'
                accept="image/*"
                id={props.name}
                type="file"
                capture="environment"
                onChange={(e) => handleCapture(e.target)}
            />

            {/* Upload Btn */}
            <label
                htmlFor={props.name}
                className='w-full'
            >
                <div
                    aria-label="upload picture"
                    className={`p-2 border-primary border-2 rounded-xl w-full
                        text-primary font-bold text-sm text-center
                    cursor-pointer hover:bg-primary-50 duration-300
                    overflow-hidden`
                    }
                >
                    {props.label}
                </div>
            </label>

            {/* Preview */}
            {
                file &&
                <Dialog
                    open={previewOpen}
                    onClose={() => setPreviewOpen(false)}
                >
                    <IconButton
                        onClick={() => setPreviewOpen(false)}
                    >
                        <Close />
                    </IconButton>
                    <img src={file.blob} />
                </Dialog>
            }
        </section >
    );
}
