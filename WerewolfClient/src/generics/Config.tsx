
interface Branch {
    minLevel: number,
    maxLevel: number,
    name: string
}

const ENV: number = 0;// 0 = dev, 1 = test, 2 = live
const DATEFORMAT: string = "YYYY-MM-DD"
const VERSION = "2.1.0"


const LIST_HEIGHT = "max-h-[calc(100vh-280px)]"

const SECTION_COLORS: Array<string> = ["#bb6228", "#fbaa19", "#2f9e49", "#79242f", "#ed1c24", '#002747']
const SECTIONS: Array<string> = ['Joey', 'Cub', 'Scout', 'Venturer', 'Rover']

const OAS_STREAMS: Array<string> = ["Bushcraft", "Bushwalking", "Camping", "Alpine", "Cycling", "Vertical", "Aquatics", "Boating", "Paddling"]
const OAS_BRANCHES: { [key: string]: Array<Branch> } = {
    Bushcraft: [
        { minLevel: 1, maxLevel: 3, name: "Bushcraft" },
        { minLevel: 4, maxLevel: 9, name: "Pioneering" },
        { minLevel: 4, maxLevel: 9, name: "Survival-skills" },
    ],
    Bushwalking: [
        { minLevel: 1, maxLevel: 9, name: "Bushwalking" },
    ],
    Camping: [
        { minLevel: 1, maxLevel: 9, name: "Camping" },
    ],
    Alpine: [
        { minLevel: 1, maxLevel: 3, name: "Alpine" },
        { minLevel: 4, maxLevel: 9, name: "Cross-country-skiing" },
        { minLevel: 4, maxLevel: 9, name: "Snow-camping-and-hiking" },
        { minLevel: 4, maxLevel: 9, name: "Downhill-skiing" },
        { minLevel: 4, maxLevel: 9, name: "Snowboarding" },
    ],
    Cycling: [
        { minLevel: 1, maxLevel: 3, name: "Cycling" },
        { minLevel: 4, maxLevel: 9, name: "Cycle-touring" },
        { minLevel: 4, maxLevel: 9, name: "Mountain-biking" },
    ],
    Vertical: [
        { minLevel: 1, maxLevel: 3, name: "Vertical" },
        { minLevel: 4, maxLevel: 9, name: "Abseiling" },
        { minLevel: 4, maxLevel: 9, name: "Canyoning" },
        { minLevel: 4, maxLevel: 9, name: "Caving" },
        { minLevel: 4, maxLevel: 9, name: "Climbing" },
    ],
    Aquatics: [
        { minLevel: 1, maxLevel: 3, name: "Aquatics" },
        { minLevel: 4, maxLevel: 6, name: "Life-saving" },
        { minLevel: 7, maxLevel: 9, name: "Swift-water-rescue" },
        { minLevel: 4, maxLevel: 6, name: "Snorkelling" },
        { minLevel: 7, maxLevel: 9, name: "Scuba" },
        { minLevel: 4, maxLevel: 9, name: "Surfing" },
    ],
    Boating: [
        { minLevel: 1, maxLevel: 3, name: "Boating" },
        { minLevel: 4, maxLevel: 9, name: "Sailing" },
        { minLevel: 4, maxLevel: 9, name: "Windsurfing" },
    ],
    Paddling: [
        { minLevel: 1, maxLevel: 3, name: "Paddling" },
        { minLevel: 4, maxLevel: 9, name: "Canoeing" },
        { minLevel: 7, maxLevel: 9, name: "White-water-rafting" },
        { minLevel: 4, maxLevel: 6, name: "Kayaking" },
        { minLevel: 7, maxLevel: 9, name: "White-water-kayaking" },
        { minLevel: 4, maxLevel: 9, name: "Sea-kayaking" },
    ],
}

const OAS_STREAM_COLORS: Array<string> = ["#40a75e", "#874149", "#5798c3"]//Core - Land - Water
const OAS_COLORS: Array<string> = ["#09361e", "#193a1e", "#106236", "#00897b", "#00acc1", "#00838f", "#2549a8", "#16337f", "#071e57"]

const SOUTH_WEST_REGIONS: Array<string> = ["WEST COAST REGION", "GEELONG REGION", "WESTERN REGION"]
const MURRAY_MIDLANDS_REGIONS: Array<string> = ["LODDON MALLEE REGION", "NORTHERN REGION"]

const SIA_AREAS = new Map<string, string>([
    ["sia_environment", "Environment"],
    ["sia_stem_innovation", "STEM & Innovation"],
    ["sia_growth_development", "Growth & Development"],
    ["sia_adventure_sport", "Adventure & Sport"],
    ["sia_better_world", "Creating a better world"],
    ["sia_art_literature", "Art & Literature"]
])

const ALL_ACHIEVEMENTS = new Map<string, string>([
    ["intro_scouting", "Introduction to Scouting"],
    ["intro_section", "Introduction to Section"],
    ["milestone", "Milestone"],
    ["outdoor_adventure_skill", "OAS"],
    ["special_interest_area", "SIA"],
    ["additional_award", "Additional Award"]
])

export {
    ENV,
    VERSION,
    LIST_HEIGHT,
    DATEFORMAT,
    SECTION_COLORS,
    SECTIONS,
    OAS_COLORS,
    OAS_STREAMS,
    OAS_BRANCHES,
    OAS_STREAM_COLORS,
    SIA_AREAS,
    ALL_ACHIEVEMENTS,
    SOUTH_WEST_REGIONS,
    MURRAY_MIDLANDS_REGIONS
};












