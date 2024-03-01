import { makeRequest } from "./makeRequest";

export function loginRequest({ email, password }) {
  return makeRequest(`identity/login`, {
    method: "POST",
    data: { email, password },
  });
}
