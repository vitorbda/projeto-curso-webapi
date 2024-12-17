import axios from 'axios';

const api = axios.create({
    baseURL: 'https://localhost:7054/'
});

export default api;