import Axios from "axios";

// TODO - fix env var
const api = Axios.create({
  baseURL: "http://localhost:5000/api",
  withCredentials: true,
});

export function makeRequest(url, options) {
  return api(url, options)
    .then((res) => res.data)
    .catch((error) =>
      Promise.reject(error?.response?.data?.message ?? "Error")
    );
}
