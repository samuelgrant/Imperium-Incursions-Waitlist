import React, { Component } from 'react';
import { render } from 'react-dom';
import { NewFleetModal, NewFleetLink } from './components/newfleets';
import Alert from './Components/alert';
import FleetInfo from './Components/FleetInfo';
import { setInterval } from 'timers';
import WaitlistUp from './Components/WaitlistUp';

const baseUri = "/waitlist";

export default class Index extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fleets: null,
            fcOptions: null,
            userOptions: null
        }
    }

    componentDidMount() {
        this.getSettings()
        this.getFleets();
        setInterval(() => this.getFleets(), 1000 * 10);
    }

    getFleets() {
        $.ajax({
            type: 'get',
            url: `/waitlist/fleets`
        }).done((fleets) => {
            this.setState({ fleets: fleets });
        }).fail((err) => {
            console.error(`React/Index {Index@getFleets} - Error getting the available fleets`, err);
        })  
    }

    getSettings() {
        $.ajax({
            type: 'get',
            url: `/api/v1/fc-settings`,
            async: false
        }).done((settings) => {
            this.setState({ fcOptions: (settings != "") ? settings : null});
        }).fail((err) => {
            console.error(`React/Index {Index@getSettings} - Error getting the FC settings`, err.responseText);
        })

        $.ajax({
            type: 'get',
            url: `/api/v1/user-settings`,
            async: false
        }).done((settings) => {
            this.setState({ userOptions: settings });
        }).fail((err) => {
            console.error(`React/Index {Index@getSettings} - Error getting the user settings`, err.responseText);
        })
    }

    availableFleets() {
        return this.state.fleets && this.state.fleets.length > 0;
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

        let newFleet = { btn: null, modal: null };
        if (this.state.fcOptions) {
            newFleet.btn = <NewFleetLink />;
            newFleet.modal = <NewFleetModal options={this.state.fcOptions} />;
        }

        return (
            <div className="container">
                <div className="row">
                    <div className="col-12">
                        <h2>Fleet Info</h2>
                    </div>

                    {noFleets}

                    {fleets}

                    {newFleet.btn}
                </div>

                <div className="row">
                    <div className="col-lg-8 col-md-12">
                        <WaitlistUp options={this.state.userOptions} />
                    </div>

                    <div className="col-lg-4 col-sm-12">
                        Queue
                    </div>
                </div>

                {newFleet.modal}
            </div>
        )
    }
}



if (document.getElementById('index'))
    render(<Index />, document.getElementById('index'));