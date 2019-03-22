import React from 'react';
import { render } from 'react-dom';

const App = () => (
    <div>
        <h1>React in ASP.NET MVC!</h1>
        <div>Hello React World</div>


        <a href="/auth/gice">Start Gice SSO</a>
        <i className="fas fa-user" />
        &nbsp; | &nbsp;
        <a href="/auth/eve">Start Eve SSO</a>
        &nbsp; | &nbsp;
        <a href="/pilot-select">Pilot Select</a>
    </div>
);

render(<App />, document.getElementById('app'));
