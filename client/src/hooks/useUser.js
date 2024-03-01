import { useIdentity } from "../contexts/IdentityContext";
// import jwt from "jsonwebtoken";

export function useUser() {
  const { accessToken } = useIdentity();
  // const secretKey = import.meta.env.VITE_JWT_SECRET;
  // console.log(secretKey);

  // const decodedToken = jwt.verify(accessToken, secretKey);
  // console.log(decodedToken);
  // return { id: document.cookie.match(/userId=(?<id>[^;]+);?$/).groups.id };
}
