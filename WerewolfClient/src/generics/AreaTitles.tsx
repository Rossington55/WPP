/*
    PROPS
    --------------
      
 */

import React, { useContext } from 'react';
import { KeyboardArrowRight } from '@mui/icons-material';
import { SettingsContext } from '../main';

interface Props {
    topSelectableLevel?: string,//The highest selectable area [branch, region, district]
}

export default function AreaTitles(props: Props) {
    const globalSettings = useContext(SettingsContext)
    const branch = globalSettings.settings.branch.label
    const region = globalSettings.settings.region.label
    const district = globalSettings.settings.district.label
    const section = globalSettings.settings.section.label

    function getAreaTitle(title: string, showArrow: boolean, selectable: boolean, onClick: () => void): React.ReactNode {
        if (title === "All") {
            return
        }

        return <section className='items-center'>
            {showArrow &&
                <KeyboardArrowRight />
            }

            <h2
                className={`text-lg max-w-[150px] text-ellipsis whitespace-nowrap ${selectable && "underline cursor-pointer"}`}
                onClick={() => {
                    if (selectable) {
                        onClick()
                    }
                }}
            >
                {title}
            </h2>
        </section>
    }

    return <section className='gap-2 align-middle flex-wrap'>
        {/* Branch */}
        {getAreaTitle(
            branch,//Label
            false,//Arrow
            region !== "All" && !["district", "region"].includes(props.topSelectableLevel ?? ""),//Selectable
            () => {
                globalSettings.setSettings([
                    { setting: "region", option: "All" },
                    { setting: "district", option: "All" },
                ])
            }
        )}

        {/* Region */}
        {getAreaTitle(
            region,//Label
            region !== "All",//Arrow
            district !== "All" && !["district"].includes(props.topSelectableLevel ?? ""),//Selectable
            () => {
                globalSettings.setSettings([
                    { setting: "district", option: "All" },
                ])
            }
        )}

        {/* District */}
        {getAreaTitle(
            district,//Label
            district !== "All",//Arrow
            false,//Selectable
            () => {
            }
        )}

        {section !== "All" &&
            <section className='items-center'>

                <h2 className='text-xl'>({section})</h2>
            </section>
        }
    </section>
}
