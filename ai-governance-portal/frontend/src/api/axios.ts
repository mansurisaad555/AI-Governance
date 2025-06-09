// src/api/axios.ts
import axios from 'axios';

const BACKEND_URL = import.meta.env.VITE_BACKEND_URL || 'http://localhost:5056';

export default axios.create({
  baseURL: BACKEND_URL,
});
