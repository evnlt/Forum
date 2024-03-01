import Axios from "axios";

const api = Axios.create({
  baseURL: import.meta.env.VITE_REACT_APP_SERVER_URL,
  withCredentials: true,
});

export function setAuthHeader(token) {
  api.interceptors.request.use(
    (config) => {
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
      return config;
    },
    (error) => {
      return Promise.reject(error);
    }
  );
}

export function makeRequest(url, options) {
  return api(url, options)
    .then((res) => res.data)
    .catch((error) =>
      Promise.reject(error?.response?.data?.message ?? "Error")
    );
}
