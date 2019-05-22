import React, { Component } from 'react';
import { render } from 'react-dom';
import Alert from './components/alert';
import { NewFleetModal, NewFleetLink } from './components/newfleets';

const baseUri = "/waitlist";

export default class Index extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fleets: null,
            fcOptions: null
        }
    }

    componentDidMount() { this.getFleets(); this.getFcSettings() }

    getFleets() {
        
    }

    getFcSettings() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/fcsettings`,
        }).done((settings) => {
            this.setState({ fcOptions: settings });
        }).fail((err) => {
            console.error(`React/Index {Index@getFcSettings} - Error getting the FC settings`, err.responseText);
        })
    }

    
    availableFleets() {
        return this.state.fleets != null;
    }

    render() {
        let noFleets;
        if (!this.availableFleets()) {
            noFleets = <Alert type="danger" ><span className="font-weight-bold">Waitlist Inactive:</span> There is either no fleet, or the waitlist is inactive. Check our in game channel for more information.</Alert>
        }

        let publicFleets;

        return (
            <div className="container">
                <div className="row">
                    <h2>Fleet Info</h2>
                    {noFleets}

                    {publicFleets}

                    <NewFleetLink />
                </div>
                <div className="row">
                    Waitlist up and queue
                </div>

                <NewFleetModal options={this.state.fcOptions}/>
            </div>
        )
    }
}



if (document.getElementById('index'))
    render(<Index />, document.getElementById('index'));