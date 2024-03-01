import React, { useState, useEffect, useContext } from "react";
import { loginRequest } from "../services/identity";

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
