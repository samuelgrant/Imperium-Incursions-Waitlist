import React, { Component } from 'react';
import { render } from 'react-dom';
import Alert from './Components/Alert';
import { SidePanel, SideSection, SidePanelButton } from './Components/SidePanel';
import { XmppLink } from './Components/CommLinks';
import { Pilot } from './Components/EsiUi';
import { BtnClose, BtnClear, BtnInvAll, BtnInvFaxes, Mumble, Status, Type } from './Components/FleetSettings';

const baseUri = "/fleets";

export default class Index extends Component {
    constructor(props) {
        super(props);

        this.state = {
            fcOptions: null,
            fleetId: null
        }
    }

    componentDidMount() {
        this.getFcSettings();

        this.setState({
            fleetId: $("#fleetManagement").data("fleetid")
        }, () => this.getData());
    }

    isPublic() {
        return (this.state.fleet) ? this.state.fleet.isPublic : null;
    }

    getData() {
        $.ajax({
            type: 'get',
            url: `${baseUri}/${this.state.fleetId}/data`,
        }).done((result) => {
            this.setState({ fleet: result });
        }).fail((err) => {
            console.error(`React/FleetManagement {FleetManagement@getData} - Error getting fleet information`, err.responseText);
        });
    }

    getFcSettings() {
        $.ajax({
            type: 'get',
            url: `/waitlist/fcsettings`,
            async: false
        }).done((settings) => {
            this.setState({ fcOptions: settings });
        }).fail((err) => {
            console.error(`React/FleetManagement {Index@getFcSettings} - Error getting the FC settings`, err.responseText);
        })
    }

    getFleetSettings() {
        return this.state.fleet || null;
    }

    getSettings() {
        return this.state.fcOptions || null;
    }

    render() {
        let fleetPrivate;
        if (!this.isPublic()) {
            fleetPrivate = (
                <Alert type="danger">
                    <span className="font-weight-bold">Fleet Not Listed: </span>
                    If no fleets are listed, the waitlist will show as offline.
                </Alert>
            );
        }

        let boss_myPilots;
        if (this.state.fcOptions) {
            boss_myPilots = this.state.fcOptions.pilots.map((pilot) => {
                return <a className="dropdown-item" role="presentation">{pilot.name}</a>
            });
        }

        let backSeat_myPilots;
        if (this.state.fcOptions) {
            backSeat_myPilots = this.state.fcOptions.pilots.map((pilot) => {
                return <a className="dropdown-item" role="presentation">{pilot.name}</a>
            });
        }
        

        return (
            <div className="container">
                {fleetPrivate}
                <SidePanelButton id="fleetSettings" title="Fleet Settings" />
                <SidePanelButton id="fleetCynos" title="Fleet Cynos" />

                <SidePanel id="fleetSettings" title="Fleet Settings">
                    <div className="row">
                        <SideSection title="Fleet Commander">
                            <div className="row mumble">
                                <div className="col-3">
                                    <img className="ml-3 pr-2" src={`https://image.eveonline.com/Character/${96304094}_64.jpg`} />
                                </div> 
                                <div className="col-9">
                                    <Pilot pilot={(this.state.fleet) ? this.state.fleet.bossPilot : null} />
                                

                                    <div className="dropdown">
                                        <button class="btn btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">I'm the Boss....</button>
                                        <div class="dropdown-menu" role="menu">
                                            {boss_myPilots}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </SideSection>

                        <SideSection title="Backseat">
                            <div className="row mumble">
                                <div className="col-3">
                                    <img className="ml-3 pr-2" src={`https://image.eveonline.com/Character/${96304094}_64.jpg`} />
                                </div>
                                <div className="col-9">
                                    <XmppLink AuthName={(this.state.fleet) ? this.state.fleet.bossPilot.name : null} />
                                    <i className="fas fa-times-circle clear"></i>


                                    <div className="dropdown">
                                        <button class="btn btn-dark dropdown-toggle" data-toggle="dropdown" aria-expanded="false" type="button">I'm the Backseat....</button>
                                        <div class="dropdown-menu" role="menu">
                                            {backSeat_myPilots}
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </SideSection>

                        <Mumble channel={(this.getFleetSettings()) ? this.getFleetSettings().commChannel : null}
                            options={(this.getSettings()) ? this.getSettings().comms : null}
                            u={this.getData.bind(this)}
                            fleetId={this.state.fleetId} />

                        <Type type={(this.getFleetSettings()) ? this.getFleetSettings().type : ""}
                            options={(this.getSettings()) ? this.getSettings().fleetTypes : null}
                            u={this.getData.bind(this)}
                            fleetId={this.state.fleetId} />

                        <Status public={this.isPublic()}
                            u={this.getData.bind(this)}
                            fleetId={this.state.fleetId} />
                    </div>

                    <hr />

                    <div className="row">
                        <BtnClose fleetId={this.state.fleetId} />
                        <BtnClear fleetId={this.state.fleetId} />

                        <BtnInvAll fleetId={this.state.fleetId} />
                        <BtnInvFaxes fleetId={this.state.fleetId} />
                    </div>
                </SidePanel>
                
                    
                <SidePanel id="fleetCynos" title="Fleet Cynos">

                </SidePanel>
            </div>
        )
    }
}



if (document.getElementById('fleetManagement'))
    render(<Index />, document.getElementById('fleetManagement'));