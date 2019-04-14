import React from 'react';
import { render } from 'react-dom';

const Bans = () => (
    <div>
        <h1>Bans Management!</h1>
        <div>Leadership Read/Write, Senior FCs read only</div>
    </div>
);

if (document.getElementById('bans'))
    render(<Bans />, document.getElementById('bans'));