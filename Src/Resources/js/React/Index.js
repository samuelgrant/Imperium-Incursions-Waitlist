import React, { Component } from 'react';
import { render } from 'react-dom';
import { NewFleetModal, NewFleetLink } from './components/newfleets';
import Alert from './components/alert';
import FleetInfo from './Components/FleetInfo';

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
        $.ajax({
            type: 'get',
            url: `/waitlist/fleets`,
        }).done((fleets) => {
            this.setState({ fleets: fleets });
        }).fail((err) => {
            console.error(`React/Index {Index@getFleets} - Error getting the available fleets`, err.responseText);
        })  
    }

    getFcSettings() {
        $.ajax({
            type: 'get',
            url: `/api/v1/fc-settings`,
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

        let fleets;
        if (this.state.fleets) {
            fleets = this.state.fleets.map((fleet, index) => {
                return <FleetInfo fleet={fleet} key={index} myPilots={(this.state.fcOptions) ? this.state.fcOptions.pilots : null} />
            })
        }

        return (
            <div className="container">
                <div className="row">
                    <div className="col-12">
                        <h2>Fleet Info</h2>
                    </div>

                    {noFleets}

                    {fleets}

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