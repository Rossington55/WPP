/*
    PROPS
    --------------
      
 */

import React, { useContext } from 'react';
import { RoleContext, Team } from './Game';

export default function MyRole() {
    const role = useContext(RoleContext)


    function getColor() {
        if (!role) { return }

        switch (role.team) {
            case Team.Villager:
                return "bg-blue"
            case Team.Werewolf:
                return "bg-red"
            case Team.Tanner:
                return "bg-yellow"
            case Team.Vampire:
                return "bg-purple"
        }
    }

    if (!role) { return }

    return (
        <article className={`card ${getColor()}`}>
            <h1>Your role is {role.name}</h1>
            <h2>Team: {Team[Number(role.team)]}</h2>
            <h2>Aim: {role.description}</h2>

        </article>
    );
}
