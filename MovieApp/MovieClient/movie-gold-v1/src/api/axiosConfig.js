import axios from 'axios'

export default axios.create({
    baseURL:'  https://c3cc-2604-3d09-867b-860-7812-72e7-3539-8e7c.ngrok-free.app',
    headers: {"ngrok-skip-browser-warning": "true"}
})