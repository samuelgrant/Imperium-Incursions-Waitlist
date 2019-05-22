import React, { Component } from 'react';
import { render } from 'react-dom';

const baseUri = "/fleets";

export default class Index extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fleets: null,
            fcOptions: null
        }
    }

    render() {
        return (
            <div className="container">
                test
            </div>
        )
    }
}



if (document.getElementById('fleetManagement'))
    render(<Index />, document.getElementById('fleetManagement'));