import axios from 'axios'

export default axios.create({
    baseURL:'https://aa7e-2604-3d09-867b-860-dcc6-29c8-6dc7-5ae6.ngrok-free.app',
    headers: {"ngrok-skip-browser-warning": "true"}
})