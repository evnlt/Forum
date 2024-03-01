import React, { useState, useContext } from "react";
import { loginRequest } from "../services/identity";
import { setAuthHeader } from "../services/makeRequest";

const Context = React.createContext();

export function useIdentity() {
  return useContext(Context);
}

export const IdentityProvider = ({ children }) => {
  const [currentUser, setCurrentUser] = useState();
  const [accessToken, setAccessToken] = useState(null);

  async function login(email, password) {
    const result = await loginRequest(email, password);
    setAccessToken(result.accessToken);
    setAuthHeader(result.accessToken);
    setCurrentUser(parseJwt(result.accessToken));
  }

  function parseJwt(token) {
    const base64Url = token.split(".")[1];
    const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
    const jsonPayload = decodeURIComponent(
      window
        .atob(base64)
        .split("")
        .map(function (c) {
          return "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join("")
    );

    const claims = JSON.parse(jsonPayload);

    return { id: claims.id, email: claims.email };
  }

  return (
    <Context.Provider value={{ accessToken, currentUser, login }}>
      {children}
    </Context.Provider>
  );
};

// const PrivateRoute = ({ component: Component, ...rest }) => {
//   const { user } = React.useContext(Context);

//   return (
//     <Route
//       {...rest}
//       render={(props) =>
//         user ? (
//           <Component {...props} />
//         ) : (
//           <Redirect
//             to={{ pathname: "/login", state: { from: props.location } }}
//           />
//         )
//       }
//     />
//   );
// };
