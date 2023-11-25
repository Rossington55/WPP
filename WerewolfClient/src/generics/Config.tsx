
export interface RoleButton {
    className?: string,
    label: string,
    returnString?: string,
    requiredSelections?: number
}

export interface RoleNightMedia {
    name: Array<string>,
    buttons: Array<RoleButton>,
    nightImage?: string,
}


export const ALL_ROLE_MEDIA: Array<RoleNightMedia> = [
    {
        name: ["Villager"],
        buttons: [],
        nightImage: "Cottage"
    },
    {
        name: ["Werewolf"],
        buttons: [
            {
                label: "Bite",
            }
        ],
        nightImage: "Doorstep"
    },
    {
        name: ["Seer", "Apprentice Seer", "Aura Seer", "Sorceress", "Mystic Seer"],
        buttons: [
            {
                label: "Search",
            }
        ]
    },
    {
        name: ["Revealer"],
        buttons: [
            {
                label: "Search",
            },
            {
                label: "Dont Search",
                returnString: "None",
                requiredSelections: 0
            }
        ]
    },
    {
        name: ["Bodyguard"],
        buttons: [
            {
                label: "Protect"
            }
        ]
    },
    {
        name: ["Priest"],
        buttons: [
            {
                label: "Protect"
            },
            {
                label: "No prayers tonight",
                returnString: "None",
                requiredSelections: 0,
            }
        ]
    },
    {
        name: ["Doppelganger"],
        buttons: [
            {
                label: "Copy DNA"
            }
        ]
    },
    {
        name: ["Witch"],
        buttons: [
            {
                label: "Health",
                returnString: "Health"
            },
            {
                label: "Poison",
                returnString: "Poison"
            },
            {
                label: "Dont use potion",
                returnString: "None",
                requiredSelections: 0
            }
        ]
    },
    {
        name: ["Cupid"],
        buttons: [
            {
                label: "Star Cross",
                requiredSelections: 2
            }
        ]
    },
    {
        name: ["Spellcaster"],
        buttons: [
            {
                label: "Silence"
            }
        ]
    },
    {
        name: ["Old Hag"],
        buttons: [
            {
                label: "Shun"
            }
        ]
    },
    {
        name: ["Cult Leader"],
        buttons: [
            {
                label: "Add to Cult"
            }
        ]
    },
    {
        name: ["Huntress"],
        buttons: [
            {
                label: "Murder"
            }
        ]
    },
    {
        name: ["Drunk"],
        buttons: [
            {
                label: "Sleep it off",
                requiredSelections: 0,
            }
        ]
    },
    {
        name: ["Minion", "Mason"],
        buttons: [
            {
                label: "Remind",
                requiredSelections: 0,
            }
        ]
    },
    {
        name: ["Mentalist"],
        buttons: [
            {
                label: "Search",
                requiredSelections: 2
            }
        ]
    },
]







