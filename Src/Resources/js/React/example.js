import React from 'react';
import { render } from 'react-dom';

const App = () => (
    <div>
        <h1>React in ASP.NET MVC!</h1>
        <div>Hello React World</div>
    </div>
);

if(document.getElementById('app'))
    render(<App />, document.getElementById('app'));
