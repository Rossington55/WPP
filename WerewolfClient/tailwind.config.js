/** @type {import('tailwindcss').Config} */
const withMT = require("@material-tailwind/react/utils/withMT");

module.exports = withMT({
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    colors: {
      'blue': {
        DEFAULT: '#172297',
        50: '#7B84EB',
        100: '#6974E8',
        200: '#4653E3',
        300: '#2232DD',
        400: '#1C2ABA',
        500: '#172297',
        600: '#121B78',
        700: '#0E1459',
        800: '#090D3A',
        900: '#04061B',
        950: '#02030C'
      },
      'red': {
        DEFAULT: '#971722',
        50: '#EB7B84',
        100: '#E86974',
        200: '#E34653',
        300: '#DD2232',
        400: '#BA1C2A',
        500: '#971722',
        600: '#6F1119',
        700: '#470B10',
        800: '#200507',
        900: '#000000',
        950: '#000000'
      },
      'green': {
        DEFAULT: '#229717',
        50: '#84EB7B',
        100: '#74E869',
        200: '#53E346',
        300: '#32DD22',
        400: '#2ABA1C',
        500: '#229717',
        600: '#1C7C13',
        700: '#16620F',
        800: '#10470B',
        900: '#0A2D07',
        950: '#072005'
      },
    },
    fontFamily: {
      custom1: ["wwFont", "sans-serif"],
      custom2: ["ghost", "sans-serif"],
    },
    extend: {
      backgroundImage: {
        'gradient-radial': 'radial-gradient(var(--tw-gradient-stops))',
      }
    }
  },
  plugins: [],
})

