import { makeRequest } from "./makeRequest";

export function getPosts() {
  let posts = makeRequest("/posts");
  return posts;
}
