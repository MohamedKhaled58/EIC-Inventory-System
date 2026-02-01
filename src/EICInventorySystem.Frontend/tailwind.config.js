/** @type {import('tailwindcss').Config} */
export default {
    content: [
        "./index.html",
        "./src/**/*.{js,ts,jsx,tsx}",
    ],
    theme: {
        extend: {
            colors: {
                primary: {
                    50: '#f5f7f5',
                    100: '#eef2ee',
                    200: '#dce5dc',
                    300: '#bccdbc',
                    400: '#94ad94',
                    500: '#738f73', // Camouflage Green base
                    600: '#587058',
                    700: '#465946',
                    800: '#3a473a',
                    900: '#313a31',
                },
                secondary: {
                    50: '#f8f7f4',
                    100: '#f0efe7',
                    200: '#e0ded1',
                    300: '#c5c2a8',
                    400: '#a8a482',
                    500: '#8f8b63', // Khaki/Sand
                    600: '#736f4d',
                    700: '#5e5a40',
                    800: '#4f4c38',
                    900: '#434033',
                },
                // Commander's Reserve Gold Color (unchanged, fits military)
                reserve: {
                    50: '#fefce8',
                    100: '#fef9c3',
                    200: '#fef08a',
                    300: '#fde047',
                    400: '#facc15',
                    500: '#eab308',
                    600: '#ca8a04',
                    700: '#a16207',
                    800: '#854d0e',
                    900: '#713f12',
                },
                danger: {
                    50: '#fef2f2',
                    100: '#fee2e2',
                    200: '#fecaca',
                    300: '#fca5a5',
                    400: '#f87171',
                    500: '#ef4444',
                    600: '#dc2626',
                    700: '#b91c1c',
                    800: '#991b1b',
                    900: '#7f1d1d',
                },
                success: {
                    50: '#f0fdf4',
                    100: '#dcfce7',
                    200: '#bbf7d0',
                    300: '#86efac',
                    400: '#4ade80',
                    500: '#22c55e',
                    600: '#16a34a',
                    700: '#15803d',
                    800: '#166534',
                    900: '#14532d',
                },
            },
            fontFamily: {
                sans: ['Cairo', 'Arial', 'sans-serif'],
            },
        },
    },
    plugins: [],
}
