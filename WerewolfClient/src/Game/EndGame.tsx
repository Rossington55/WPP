/*
    PROPS
    --------------
      
 */

import React, { useContext } from 'react';
import { RoleContext, Team } from './Game';
interface Props {
    winner: string
}

export default function EndGame(props: Props) {
    const role = useContext(RoleContext)
    return (
        <article>
            {Team[Number(role?.team)] === props.winner ?
                <article>
                    {/* Winner */}
                    <h1 className='text-[#55ff55]'>You Won!</h1>
                </article>
                :
                <article>
                    {/* Loser */}
                    <h1 className='text-[#ff5555]'>You Lost</h1>
                </article>

            }
        </article>
    );
}
