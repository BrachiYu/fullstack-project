import axios from 'axios';

axios.defaults.baseURL = "https://localhost:7271";

axios.interceptors.response.use(
  response => {
    return response;
  },
  error => {
    console.error('API Error:', error.response ? error.response.data : error.message);
    return Promise.reject(error);
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get("/items");
    return result.data;
  },

  addTask: async (name) => {
    console.log('addTask', name);
    const result = await axios.post("/items", { name, isComplete: false });
    return result.data;
  },

  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', { id, isComplete });
    const result = await axios.put(`/items/${id}`, { isComplete });
    return result.data;
  },

  deleteTask: async (id) => {
    console.log('deleteTask');
    await axios.delete(`/items/${id}`);
  }
};
