import React, { Component } from 'react';
import { render } from 'react-dom';
import { NewFleetModal, NewFleetLink } from './components/newfleets';
import Alert from './Components/alert';
import FleetInfo from './Components/FleetInfo';
import { setInterval } from 'timers';
import WaitlistUp from './Components/WaitlistUp';
import WaitingPilot from './Components/WaitingPilots';


const baseUri = "/";

export default class Index extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fcOptions: null,
            fleets: null,
            pilots: null,
            prefPilot: null
        }
    }

    componentDidMount() {
        this.getFleets();

        setInterval(() => this.getFleets(), 1000 * 10);
    }

    getData() {
        $.ajax({
            type: 'get',
            url: `/api/v1/options`,
        }).done((data) => {
            this.setState({
                prefPilot: data.prefPilot,
                fcOptions: data.fcOptions ? data.fcOptions : null
            });
        }).fail((err) => {
            console.error(`[React/Index@getData] Error getting user options: ${err.responseText}`)
        })
    }

    getFleets() {
        $.ajax({
            type: 'get',
            url: `${baseUri}waitlist/data`,
        }).done((data) => {
            this.setState({
                fleets: data.fleets,
                options: data.options,
                pilots: data.pilots
            });

            this.getData();
        }).fail((err) => {
            console.error(`[React/Index@getFleets] Error getting fleets: ${err.responseText}`)
        })
    }

    availableFleets() {
        return this.state.fleets && this.state.fleets.length > 0;
    }

    availablePilots() {
        return this.state.pilots ? this.state.pilots.avaliable : null;
    }

    waitingPilots() {
        return this.state.pilots ? this.state.pilots.waiting : null;
    }

    render() {
        let fleets;
        if (this.state.fleets) {
            fleets = this.state.fleets.map((fleet, index) => {
                return <FleetInfo fleet={fleet} key={index} showFcOptions={this.state.fcOptions ? true : false } myPilots={(this.state.fcOptions) ? this.state.fcOptions.pilots : null} />
            })
        }

        let newFleet = { btn: null, modal: null };
        if (this.state.fcOptions) {
            newFleet.btn = <NewFleetLink />;
            newFleet.modal = <NewFleetModal options={this.state.fcOptions} prefPilot={this.state.prefPilot} />;
        }

        let noFleets;
        if (!this.availableFleets()) {
            noFleets = <Alert type="danger" ><span className="font-weight-bold">Waitlist Inactive:</span> There is either no fleet, or the waitlist is inactive. Check our in game channel for more information.</Alert>
        }

        let waitlistUi;
        if (this.availableFleets()) {
            waitlistUi = (
                <div className="row">
                    <div className="col-lg-4 col-md-6 col-sm-12">
                        <WaitlistUp options={this.state.options} pilots={this.availablePilots()} prefPilot={this.state.prefPilot} baseUri={baseUri} u={this.getFleets.bind(this)} />
                    </div>

                    <div className="col-lg-4 col-md-6 col-sm-12">
                        <WaitingPilot pilots={this.waitingPilots()} baseUri={baseUri} u={this.getFleets.bind(this)} />
                    </div>

                    <div className="col-lg-4 col-sm-12">
                        Queue
                    </div>
                </div>
            )
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

                {waitlistUi}

                {newFleet.modal}
            </div>
        )
    }
}



if (document.getElementById('index'))
    render(<Index />, document.getElementById('index'));