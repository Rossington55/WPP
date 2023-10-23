interface Props {
    label: string | any
}

export default function ChartTooltip(props: Props) {
    return (

        <article className="bg-background-800 p-2 rounded-md border-2 z-10 static">
            {props.label}
        </article>
    )
}



const oas_total_progression = [
    {
        oasStream: String,//Vertical
        oasBranch: String,//Climbing
        levels: [
            {
                level: Number,//4
                sections: {
                    section: String,//Scouts
                    groups: [
                        {
                            groupName: String,
                            districtName: String,
                            regionName: String,
                            branchName: String,
                            totalAwarded: Number,//This group currently has 5 members with highest level 'Vertical 4'
                        }
                    ]
                }
            }
        ]
    }
]

