import axios from 'axios'

export default axios.create({
    baseURL:'https://fdf5-192-197-128-21.ngrok-free.app',
    headers: {"ngrok-skip-browser-warning": "true"}
})